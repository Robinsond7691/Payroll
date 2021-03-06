import React, { Fragment } from 'react';
import { Icon, Image, Pagination, Segment, Table } from 'semantic-ui-react';
import { useModalDispatch } from '../../app/context/modal/modalContext';
import EditEmployee from '../employees/EditEmployee';
import EditManagerStatus from '../managers/EditManagerStatus';
import { User } from '../../app/api/agent';
import { openModal } from '../../app/context/modal/modalActions';
import { useTimestampState } from '../../app/context/timestamps/timestampContext';

const ListManagersTable = () => {
  const pageSize = 10;
  const pageOne = 1;

  const { refresh } = useTimestampState();
  const [managers, setManagers] = React.useState([]);
  const modalDispatch = useModalDispatch();

  const [managersPagination, setManagersPagination] = React.useState(null);
  const [loading, setLoading] = React.useState(false);

  const loadManagers = React.useCallback((pageNumber) => {
    setLoading(true);
    User.getManagers(pageSize, pageNumber).then((result) => {
      setManagers(result.data);
      setManagersPagination(JSON.parse(result.headers['x-pagination']));
      setLoading(false);
    });
  }, []);

  React.useEffect(() => {
    loadManagers(pageOne);
  }, [loadManagers, refresh]);

  const jobsitePageChangeHandler = (e, { activePage }) => {
    if (managersPagination.HasNext || managersPagination.HasPrevious) {
      loadManagers(activePage);
    }
  };

  if (loading)
    return (
      <Segment loading={loading}>
        <Image src='/assets/paragraph.png' />
      </Segment>
    );

  return (
    <Fragment>
      <Table selectable>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell>Name</Table.HeaderCell>
            <Table.HeaderCell>Username</Table.HeaderCell>
            <Table.HeaderCell>Email</Table.HeaderCell>
            <Table.HeaderCell width={2}>Manage</Table.HeaderCell>
          </Table.Row>
        </Table.Header>

        <Table.Body>
          {managers.map((manager) => {
            return (
              <Table.Row key={manager.username}>
                <Table.Cell>{manager.displayName}</Table.Cell>
                <Table.Cell>{manager.username}</Table.Cell>
                <Table.Cell>{manager.email}</Table.Cell>
                <Table.Cell>
                  {' '}
                  <Icon
                    name='edit'
                    color='blue'
                    style={{ cursor: 'pointer' }}
                    onClick={() =>
                      openModal(
                        <EditEmployee
                          username={manager.username}
                          manager={true}
                        />,
                        modalDispatch
                      )
                    }
                  />
                  <Icon
                    name='cancel'
                    color='red'
                    style={{ marginLeft: '10px', cursor: 'pointer' }}
                    onClick={() =>
                      openModal(
                        <EditManagerStatus
                          username={manager.username}
                          revoke={true}
                        />,
                        modalDispatch
                      )
                    }
                  />
                </Table.Cell>
              </Table.Row>
            );
          })}
        </Table.Body>
      </Table>
      <div style={{ width: '100%', overflow: 'auto', marginBottom: '30px' }}>
        {managersPagination && (
          <Pagination
            boundaryRange={0}
            activePage={managersPagination.CurrentPage}
            onPageChange={jobsitePageChangeHandler}
            siblingRange={1}
            totalPages={Math.ceil(
              managersPagination.TotalCount / managersPagination.PageSize
            )}
            borderless
            size='small'
            floated='right'
          />
        )}
      </div>
    </Fragment>
  );
};

export default ListManagersTable;
